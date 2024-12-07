from flask_sqlalchemy import SQLAlchemy
from app import app
from config import *

'''
This is a setup code for a Flask. This integrates and configures SQLAlchemy library. 
This creates an instance of SQLAlchemy that is tied to the Flask application (app).
'''

app.config["SQLALCHEMY_DATABASE_URI"] = database_uri
app.config["SQLALCHEMY_ENGINE_OPTIONS"] = {"isolation_level": "AUTOCOMMIT"}
db = SQLAlchemy(app)
