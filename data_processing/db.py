from sqlalchemy import create_engine, text
from os import environ

def create_database():
    DATABASE_URL = environ.get("DATABASE_URL")
    DATABASE_USERNAME = environ.get("DATABASE_USERNAME")
    DATABASE_PASSWORD = environ.get("DATABASE_PASSWORD")
    DATABASE_PORT = environ.get("DATABASE_PORT")

    database_uri = f"postgresql://{DATABASE_USERNAME}:{DATABASE_PASSWORD}@{DATABASE_URL}:{DATABASE_PORT}"

    engine = create_engine(
        database_uri,
        isolation_level="Autocommit"
        )

    try: 
        with open("schema.sql", "r") as file:
            schema = file.read()

        with engine.connect() as connection:
            connection.execute(text(schema))
            sql = text("SELECT version();")
            result = connection.execute(sql)
            version = result.fetchone()
            print(f"Connected to PostgreSQL, version: {version[0]}")

        return engine.connect()
    
    except Exception as e:
        print(e)
