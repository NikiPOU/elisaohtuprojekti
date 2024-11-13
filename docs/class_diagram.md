```mermaid
classDiagram
    class App

    class Database

    class DataReceiver
    DataReceiver : -latest_data
    DataReceiver : +get_gsi_data(data)

    class DataProcessor
    DataProcessor : +parse_data(data)

    class DatabaseUpdator
    DatabaseUpdator : +update_database(data)

    class DatabaseHandler

    class DataEncoding
    DataEncoding : +create_json_file(data)
    DataEncoding : +write_json_file(data, file_name)

    App --> Database
    DataReceiver --> DataProcessor
    Main --> DataReceiver
    Main --> App
    DataProcessor --> DataEncoding
    DataProcessor --> DatabaseUpdator
    DatabaseHandler --> DataEncoding
```
