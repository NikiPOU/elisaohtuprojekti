from flask_sqlalchemy import SQLAlchemy
from os import environ
from app import app

DATABASE_URL = environ.get("DATABASE_URL")
DATABASE_USERNAME = environ.get("DATABASE_USERNAME")
DATABASE_PASSWORD = environ.get("DATABASE_PASSWORD")
DATABASE_PORT = environ.get("DATABASE_PORT")

#database_uri = f"postgresql://{DATABASE_USERNAME}:{DATABASE_PASSWORD}@{DATABASE_URL}:{DATABASE_PORT}"
database_uri = f"postgresql+psycopg2:///mseppi"
app.config["SQLALCHEMY_DATABASE_URI"] = database_uri
app.config["SQLALCHEMY_ENGINE_OPTIONS"] = {"isolation_level": "AUTOCOMMIT"}
db = SQLAlchemy(app)
