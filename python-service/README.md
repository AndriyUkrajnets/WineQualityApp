# Python ML service

The service exposes `POST /predict` and `GET /health`. It loads the trusted
scikit-learn pipeline once at startup. Until the model artifact is present,
`/health` reports `model_ready: false` and `/predict` returns HTTP 503.

## Model artifact

Place the fitted model at `artifacts/wine_quality_random_forest.joblib` under
this directory. Set `WINE_MODEL_PATH` to use a different path. Keep the model's
metadata file alongside it for version and training details.

## Run locally

From this directory, install the requirements and start the development server:

```powershell
python -m pip install -r requirements.txt
python -m uvicorn app.main:app --reload
```

The default URL is `http://127.0.0.1:8000`.

py -m venv .venv
.\.venv\Scripts\Activate.ps1
py -m pip install -r requirements.txt
py -m uvicorn app.main:app --reload