# SmartHome_Backend

## Wetterdaten-API
Die Wetterdaten-API ist eine ASP.net-API, die es ermöglicht, Wetterdaten zu sammeln und in MongoDB zu speichern. Die API bietet auch Funktionen zum Abrufen, Aktualisieren und Hinzufügen von Wetterdaten sowie zum Abrufen von Min-, Max- und Durchschnittswerten.

## Installation
Zur Installation der Wetterdaten-API sind folgende Schritte erforderlich:

Laden Sie das Projekt aus dem Repository herunter.
Öffnen Sie das Projekt in Visual Studio.
Stellen Sie sicher, dass MongoDB installiert und ausgeführt wird.
Führen Sie das Projekt in Visual Studio aus.
Verwendung
Die Wetterdaten-API bietet die folgenden Endpunkte:

- GET api/WeatherStation: Ruft alle Wetterdaten von der Wetterstation ab.
- GET api/TempIndoor: Ruft alle Wetterdaten von der Innentemperatur ab.
- GET api/WeatherStation/Datum?dayTime=18.2.23: Ruft alle Wetterdaten von der Wetterstation an einem bestimmten Datum ab.
- GET api/TempIndoor/Datum?dayTime=18.2.23: Ruft alle Wetterdaten von der Innentemperatur an einem bestimmten Datum ab.
- POST api/WeatherStation: Sendet Wetterdaten von der Wetterstation an die API, die dann in MongoDB gespeichert werden.- 
- POST api/TempIndoor: Sendet Wetterdaten von der Innentemperatur an die API, die dann in MongoDB gespeichert werden.
- PUT api/WeatherStation/Datum?dayTime=18.2.23: Aktualisiert Wetterdaten von der Wetterstation an einem bestimmten Datum.
- PUT api/TempIndoor/Datum?dayTime=18.2.23: Aktualisiert Wetterdaten von der Innentemperatur an einem bestimmten Datum.
- GET api/WeatherAverage/Datum?dayTime=18.2.23: Ruft den Durchschnittswert aller Wetterdaten von der Wetterstation an einem bestimmten Datum ab.
- GET api/TempIndoorAverage/Datum?dayTime=18.2.23: Ruft den Durchschnittswert aller Wetterdaten von der Innentemperatur an einem bestimmten Datum ab.

Die Wetterdaten-API erwartet, dass die Wetterdaten im JSON-Format gesendet werden und folgende Felder enthalten:

temperatur: Die Temperatur in Grad Celsius.
tempMin: Die niedrigste Temperatur in Grad Celsius.
tempMax: Die höchste Temperatur in Grad Celsius.
wind: Die Windgeschwindigkeit in km/h.
windMin: Die niedrigste Windgeschwindigkeit in km/h.
windMax: Die höchste Windgeschwindigkeit in km/h.
luftfeuchtigkeit: Die Luftfeuchtigkeit in Prozent.
humidityMin: Die niedrigste Luftfeuchtigkeit in Prozent.
humidityMax: Die höchste Luftfeuchtigkeit in Prozent.
rain: Die Regenmenge in mm.
raining: True, wenn es regnet, False, wenn es nicht regnet.
sunDuration: Die Sonnenscheindauer in Stunden.
dayTime: Das Datum im Format TT.MM.JJJJ.
id: Eine eindeutige ID für jede Wetterdatenaufzeichnung.
