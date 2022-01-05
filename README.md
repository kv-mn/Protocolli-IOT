# Protocolli-IoT
## Gruppo: 
- Matteo Faraoni
- Kevin Mainardis

## COAP
### Messaggi di stato
Abbiamo implementato il metodo POST che risponde a `url/DroneStatus` con il seguente payload JSON:
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

Ad ogni richiesta rispondiamo con:
- 2.04 Changed in caso di inserimento corretto
- 4.00 Bad Request in caso di payload non compatibile
- 5.00 Internal Server Error in caso di fallimento durante il salvataggio nel database

Le richieste POST effettuate dal Drone saranno di tipo NON (se qualche messaggio viene perso non è un problema).