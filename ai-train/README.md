````markdown
# AI Forecast Service (Synthetic + SQL Server)

## Overview

This microservice trains an ML model (RandomForest) on synthetic rental & fleet data and exposes endpoints to train and forecast demand per branch. The service supports:

- Generating synthetic data (CSV or directly into SQL Server)
- Training model from CSV or SQL Server
- Forecasting next-N days demand for a given branch and returning recommendation to add vehicles

## Quickstart (local, CSV mode)

1. Create virtual env and install packages:

```bash
python -m venv .venv
source .venv/bin/activate # or .venv\Scripts\activate on Windows
pip install -r requirements.txt
```
````

2. Generate synthetic data (CSV):

```bash
python data_generator.py --out csv --out-path data/synthetic_rental_data.csv
```

3. Train model from CSV:

```bash
python app.py --train-mode csv --data-path data/synthetic_rental_data.csv
# or use the HTTP endpoint:
curl -X POST http://localhost:5005/api/ai/train -H "Content-Type: application/json" -d '{"mode":"csv","data_path":"data/synthetic_rental_data.csv"}'
```

4. Forecast for branch 1 for next 7 days (HTTP):

```bash
curl -X POST http://localhost:5005/api/ai/forecast -H "Content-Type: application/json" \
-d '{"branch_id":1, "horizon":7}'
```

## SQL Server mode

If you have SQL Server available, you can push synthetic data into it or point training to SQL Server.
Set environment variable `SQLSERVER_CONN` to a SQLAlchemy connection string, example:

```
SQLSERVER_CONN = "mssql+pyodbc://sa:YourPassword@host:1433/YourDB?driver=ODBC+Driver+17+for+SQL+Server"
```

Then generate data and write to SQL Server:

```bash
python data_generator.py --out mssql --rows 180 --db-table daily_demand
```

Train from SQL Server:

```bash
python app.py --train-mode mssql --db-table daily_demand
```

---
