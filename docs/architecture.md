# Arkkitehtuurin suunnitelma

## Arkkitehtuurikaavio
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

## Suunnitelma

### Serverin puoli
Pelistä lähetetään dataa suoraan Openshiftiin, jolloin se on sovelluksen käytettävissä. GSI processor karsii datasta muun kuin sovelluksen tarvitseman datan. Karsinnan jälkeen data siirretään joko suoraan encoding-osioon ja/tai tallennetaan tietokantaan. Encoding muuttaa datan sellaiseen muotoon, että Unity pystyy käsittelemään sitä (JSON). Tietokannasta tuleva data menee myös encodingin läpi.

*Video ja data tulevat yhteiseen timeriin, jossa videon kuva ja data saadaan synkronisoitua. Ei ajankohtainen ominaisuus*

### Clientin puoli
Client on VR-lasit ja Unity. Serveri ja Unity saavat yhteyden WebSocketin avulla, jolloin data saadaan Unitylle käytettäväksi. Unityssä tapahtuu datan renderöinti VR-ympäristöön.


