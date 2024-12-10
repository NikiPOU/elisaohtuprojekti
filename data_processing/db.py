'''
This is a setup code for a Flask. This integrates and configures SQLAlchemy library. 
Also this file creates an instance of SQLAlchemy that is tied to the Flask application (app).
'''
from flask_sqlalchemy import SQLAlchemy
from app import app
from config import *

app.config["SQLALCHEMY_DATABASE_URI"] = database_uri
app.config["SQLALCHEMY_ENGINE_OPTIONS"] = {"isolation_level": "AUTOCOMMIT"}
db = SQLAlchemy(app)
