from flask import Flask, request, jsonify
from flask_cors import CORS
import pandas as pd
import joblib
import os
from sklearn.linear_model import LinearRegression
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder


app = Flask(__name__)
CORS(app)  # Enable CORS for all routes

MODEL_PATH = "trained_model.pkl"

# -----------------------------
# TRAIN MODEL
# -----------------------------
def train_model_from_csv(csv_path):
    # Đọc dữ liệu từ file csv
    print(f"📘 Training model from: {csv_path}")
    df = pd.read_csv(csv_path)
    # -------------------------
    # Mã hóa categorical với LabelEncoder
    # -------------------------
    branch_encoder = LabelEncoder()
    vehicle_encoder = LabelEncoder()

    df["branch_encoded"] = branch_encoder.fit_transform(df["branch"])
    df["vehicle_type_encoded"] = vehicle_encoder.fit_transform(df["vehicle_type"])

    # Lưu encoder để dùng cho dự đoán
    joblib.dump(branch_encoder, "branch_encoder.pkl")
    joblib.dump(vehicle_encoder, "vehicle_encoder.pkl")

    X = df[["branch_encoded", "vehicle_type_encoded", "rental_hours", "feedback_score", "maintenance_required"]]
    y = df["price"]

    # Phân chia dữ liệu thành 2 phần: 80% học và 20% test
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

    # Sử dụng hồi quy tuyến tính để tìm ra quy luật giữa X và Y => y ≈ w0​ + w1​x1 ​+ w2​x2
    model = LinearRegression()
    model.fit(X_train, y_train)

    # Lưu dữ liệu vào model path
    joblib.dump(model, MODEL_PATH)

    # Đánh giá lại mô hình
    score = model.score(X_test, y_test)
    print(f"✅ Model trained successfully (R²: {score:.3f})")
    return score

# -----------------------------
# API ROUTES
# -----------------------------
@app.route("/api/ai/forecast", methods=["POST"])
def forecast():
    data = request.get_json()
    if not os.path.exists(MODEL_PATH):
        return jsonify({"error": "Model not trained yet"}), 400

    model = joblib.load(MODEL_PATH)
    branch_encoder = joblib.load("branch_encoder.pkl")
    vehicle_encoder = joblib.load("vehicle_encoder.pkl")

    df = pd.DataFrame([data])
    df["branch_encoded"] = branch_encoder.transform([data["branch"]])
    df["vehicle_type_encoded"] = vehicle_encoder.transform([data["vehicle_type"]])

    X = df[["branch_encoded", "vehicle_type_encoded", "rental_hours", "feedback_score", "maintenance_required"]]
    prediction = model.predict(X)[0]

    suggestion = "Giu nguyen doi xe"
    if prediction > 1000:
        suggestion = "Nen tang them xe"
    elif prediction < 300:
        suggestion = "Co the giam bot xe"

    return jsonify({
        "forecasted_rental_value": round(prediction, 2),
        "suggestion": suggestion
    })


# -----------------------------
# MAIN ENTRY
# -----------------------------
if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument("--train-mode", choices=["csv"], default=None)
    parser.add_argument("--data-path", default="synthetic_rental_data.csv")
    args = parser.parse_args()

    if args.train_mode == "csv":
        train_model_from_csv(args.data_path)
    else:
        print("🚀 Starting Flask API server...")
        app.run(debug=True, host='0.0.0.0', port=5005)
