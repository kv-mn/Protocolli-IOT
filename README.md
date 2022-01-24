# Protocolli-IoT
## Gruppo: 
- Matteo Faraoni
- Kevin Mainardis

## AMQP
Utilizziamo AMQP come buffer nel client/drone. <br>
L'exchange è di tipo `direct` e il binding con la coda è effettuato tramite routing key = `queue`. <br>
L'invio dei messaggi consumati dalla coda viene effettuato tramite MQTT come descritto di seguito.

Se volessimo utilizzare AMQP come protocollo di comunicazione tra Server e Drone useremmo: 
- Exchange di tipo direct con routing key = `{droneId}` e n code (una per drone) per l'invio dei messaggi di stato. <br>
Il server si sottoscrive a tutte le code e processa tutti i messaggi di stato.
- Exchange di tipo direct con routing key = `{droneId}` e n code (una per drone) per l'invio dei comandi. <br>
Ogni drone si sottoscrive solamente alla coda relativa ai propri comandi.

## MQTT
### Opzioni di connessione:
### Drone 
- Clean Session: false (vogliamo ricevere i comandi ricevuti mentre eravamo offline)
- Last Will Topic: `gameofdrones/{DroneId}/lastwill`
- Last Will QoS: 2 (vogliamo sapere con precisione se un drone si è disconnesso)
- Last Will Message: JSON <br>
    ```json
        {
        "DroneId": 0,
        "Error": "Unexpected exit",
        "Timestamp": 1638459750
        }
    ```
- Last Will Retain: true (se per caso il server perde la connessione al broker, vogliamo comunque sapere se un drone si è disconnesso nel frattempo)
- Keep Alive: 20 (tempo sufficientemente lungo per gestire le fluttuazioni di connessione dovute alla rete mobile)

### Server 
- Clean Session: true (non abbiamo necessità di recuperare eventuali messaggi di stato non ricevuti)
- Last Will: nessuno (non abbiamo necessità di notificare nessun client)
- Keep Alive: 5 (avendo una connessione stabile ci aspettiamo di poterci riconnettere entro poco tempo)

### Messaggi di stato
Topic: `gameofdrones/{DroneId}/status` <br>
Quality of Service: 0 (se qualche messaggio viene perso non è un problema) <br>
Retain flag: true (in modo da poter visualizzare i dati su una pagina web senza dover aspettare l'arrivo di un nuovo messaggio) <br> 
Payload: JSON
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

### Comandi
Topic: `gameofdrones/{DroneId}/commands` <br>
Quality of Service: 2 (vogliamo essere sicuri che i messaggi siano stati ricevuti una volta sola) <br>
Retain flag: false (tenere in memoria l'ultimo comando inviato può generare conflitti durante una connessione successiva) <br>
Payload: JSON
```json
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

### Sicurezza
Gestiamo la sicurezza a livello Transport con utilizzo di crittografia TLS/SSL e autenticazione tramite certificato (username e password sarebbero scomodi da gestire con un ampio numero di device). <br>
Limitiamo le autorizzazioni dei droni permettendo il publish e il subscribe ai soli topic relativi al drone stesso.
