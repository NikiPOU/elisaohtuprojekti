from config import *
from sqlalchemy import text, create_engine


engine = create_engine(database_uri, isolation_level="AUTOCOMMIT")

def database_initializer():
    with open("schema.sql", "r") as file:
        sql = file.read()


    with engine.connect() as connection:
        connection.execute(text(sql))
        print("Database initialized")



if __name__ == "__main__":
    database_initializer()
