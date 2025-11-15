import pandas as pd
import numpy as np
import random
from datetime import datetime, timedelta

def generate_rental_data(num_records=1000):
    data = []
    branches = ["Hanoi Center", "HCM District 1", "Da Nang City"]
    vehicle_types = ["Car", "Bike", "Electric Scooter"]

    for i in range(num_records):
        start_date = datetime(2025, 1, 1) + timedelta(days=random.randint(0, 300))
        rental_hours = random.randint(1, 72)
        end_date = start_date + timedelta(hours=rental_hours)

        record = {
            "rental_id": i + 1,
            "branch": random.choice(branches),
            "vehicle_type": random.choice(vehicle_types),
            "rental_start": start_date,
            "rental_end": end_date,
            "rental_hours": rental_hours,
            "price": rental_hours * random.randint(5, 20),
            "feedback_score": random.randint(1, 5),
            "maintenance_required": random.choice([0, 1]),
        }
        data.append(record)

    df = pd.DataFrame(data)
    df.to_csv("synthetic_rental_data.csv", index=False)
    print("✅ Generated synthetic_rental_data.csv with", len(df), "records")

if __name__ == "__main__":
    generate_rental_data(10000)
