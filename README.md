# Protocolli-IoT
## Gruppo: 
- Matteo Faraoni
- Kevin Mainardis

## MQTT
### Messaggi di stato
Topic: `gameofdrones/{DroneId}/status` <br>
Quality of Service: 0 <br>
Payload: JSON
```
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

### Comandi
Topic: `gameofdrones/{DroneId}/commands` <br>
Quality of Service: 2 <br>
Payload: JSON
```
{
"DroneId": 0,
"Command": "power",
"Value": true,
"Timestamp": 1638459750
}
```

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