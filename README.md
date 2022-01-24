# Protocolli-IoT
## Gruppo: 
- Matteo Faraoni
- Kevin Mainardis

## COAP
To do:
- Separare DroneStatus in path `url/DroneStatus/{droneId}`
- Separare DroneCommand in path dedicati ai singoli droni (al momento vengono restuiti i comandi relativi ad un solo drone)
- Implementare ricezione comandi nel client/drone tramite GET con opzione observe

### Messaggi di stato
Abbiamo implementato il metodo POST che risponde a `url/DroneStatus` con il seguente payload JSON:
```json
{
"DroneId": 0,
"Position":	{
    "X": 37.44742933541882,
    "Y": 89.61939236131468,
    "Z": 793.3927647738683
    },   
"Velocity": 105.075714,
"Battery": 82,
"Timestamp": 1638459750
}
```

Ad ogni richiesta rispondiamo con:
- 2.04 Changed in caso di inserimento corretto
- 4.00 Bad Request in caso di payload non compatibile
- 5.00 Internal Server Error in caso di fallimento durante il salvataggio nel database

Le richieste POST effettuate dal Drone saranno di tipo NON (se qualche messaggio viene perso non è un problema).

### Comandi
Implementato il metodo GET con opzione 'observe' che risponde a `url/DroneCommand` e restituisce il seguente payload JSON relativo ad un drone predefinito:
```json
{
"DroneId": 0,
"Command": "power",
"Value": true,
"Timestamp": 1638459750
}
```
Con l'opzione 'observe' i comandi vengono restuiti ad intervalli regolari.

I comandi e i relativi valori sono:
- Apertura nuova corsa
    - Command: "new-ride"
    - Value: true
- Chiusura corsa corrente
    - Command: "close-ride"
    - Value: true
- Accensione/Spegnimento
    - Command: "power"
    - Value: true/false
- Rientro alla base
    - Command: "return-to-base"
    - Value: true
- Accensione/Spegnimento LED di posizione
    - Command: "position-led"
    - Value: true/false
