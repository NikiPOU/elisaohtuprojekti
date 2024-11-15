from flask_sqlalchemy import SQLAlchemy
from app import app
from config import *

app.config["SQLALCHEMY_DATABASE_URI"] = database_uri
app.config["SQLALCHEMY_ENGINE_OPTIONS"] = {"isolation_level": "AUTOCOMMIT"}
db = SQLAlchemy(app)
