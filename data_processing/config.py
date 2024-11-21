from os import environ

DATABASE_URL = environ.get("DATABASE_URL")
DATABASE_USERNAME = environ.get("DATABASE_USERNAME")
DATABASE_PASSWORD = environ.get("DATABASE_PASSWORD")
DATABASE_PORT = environ.get("DATABASE_PORT")

#database_uri = f"postgresql://{DATABASE_USERNAME}:{DATABASE_PASSWORD}@{DATABASE_URL}:{DATABASE_PORT}"
database_uri = "postgresql+psycopg2:///mseppi"