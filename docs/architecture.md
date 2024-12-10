# Plan for the Architecture

## Architecture diagram
```mermaid
architecture-beta
  group server(server)[Server]
  group client(disk)[Client]

  service db(database)[Database] in server
  service gsi_processor(internet)[GSI processor] in server
  service encoding(internet)[Data encoding] in server
  service timer(internet)[Timer] in server
  service glasses(disk)[VR glasses] in client

  service game(internet)[Game]
  service video(internet)[Video]

  game:R --> L:gsi_processor
  gsi_processor:R --> L:db
  gsi_processor:B --> T:encoding
  db:B --> T:encoding
  encoding:B --> T:timer
  video:R --> L:timer
  timer:B --> T:glasses
  
```

## The plan

### Server side
The game sends data directly to Openshift where it is available to the app. GSI processor parses the data and removes unnecessary parts. After parsing the data is either moved directly to encoding and/or saved to database. Encoding changes the data into JSON so Unity can handle it. The data coming from the database also goes through the encoding.

*The video and data have a shared timer where the video feed and data can be synced. Not a current feature*

### Client side
The client includes the VR glasses and Unity. Currently Unity fetches the data from the Openshift address. *A websocket implementation as the connection was planned but not implemented.* After fetching the data, Unity renders it into the VR environment.


