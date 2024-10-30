# Tietokannan suunnittelua

Tietokannan nimi: gamedata

### playerData
Sis. kaikki pelaajat, jotka ovat tulleet peleissä vastaan (?)
  
  - (ID)
  - **Steam id**
  - Nimi
  - Tiimin nimi

### livePlayerData
Sis. käynnissä olevan pelin pelaajadata

  - (ID)
  - **Steam id**
  - T vai CT
  - Tapot, Assists, Kuolemat
  - Koordinaatit
  - Katseen suunta
  - Health

### heatmapData
Sis. jokaisen live matsin pelaajan lämpökartan

  - (ID)
  - **Steam id**
  - Jotenkin ruutujen käyntikerrat kuvattuina

### liveMatchData
Sis. tämän hetkisen matsin tilan

  - (ID)
  - Kartta
  - Kierros
  - Pistetilanne
  - Peliaika
