import pymongo
import random
import time
from datetime import datetime

def crear_documento():
    return {
        "humedad": random.randint(0, 100),
        "oxigeno": random.randint(0, 100),
        "luz": random.randint(0, 100),
        "nutrientes": random.randint(0, 100)
    }

def insertar_datos_aleatorios():
    client = pymongo.MongoClient("mongodb+srv://root:root@proyectotfg.vprqszh.mongodb.net/?retryWrites=true&w=majority&appName=ProyectoTFG")
    db = client["ProyectoTFG"]

    # Inserción inicial en la colección Laboratorio
    if db.Laboratorio.count_documents({}) == 0:
        db.Laboratorio.insert_one({
            "semilla1": "s1",
            "semilla2": "s2",
            "semilla3": "s3",
            "semilla4": "s4",
            "semilla5": "s5",
            "semilla6": "s6",
            "plantula1": "p1",
            "plantula2": "p2",
            "plantula3": "p3",
            "plantula4": "p4",
            "plantula5": "p5",
            "plantula6": "p6"
        })

    while True:
        # Climatica
        db.Climatica.insert_one({
            "hora": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            "sensor1": random.randint(0, 100),
            "sensor2": random.randint(0, 100),
            "sensor3": random.randint(0, 100),
            "sensor4": random.randint(0, 100),
            "sensor5": random.randint(0, 100),
            "sensor6": random.randint(0, 100),
            "sensor7": random.randint(0, 100),
            "sensor8": random.randint(0, 100)
        })

        # Hidraulica
        db.Hidraulica.insert_one({
            "nivel_deposito": random.randint(0, 100),
            "oxigeno": random.randint(0, 100),
            "potasio": random.randint(0, 100),
            "nitrogeno": random.randint(0, 100),
            "fosforo": random.randint(0, 100)
        })

        # Luminica
        db.Luminica.insert_one({
            "nivel1": random.randint(0, 100),
            "nivel2": random.randint(0, 100),
            "nivel3": random.randint(0, 100),
            "nivel4": random.randint(0, 100),
            "nivel5": random.randint(0, 100),
            "nivel6": random.randint(0, 100),
            "nivel7": random.randint(0, 100),
            "nivel8": random.randint(0, 100),

            "potencia1": random.randint(0, 100),
            "potencia2": random.randint(0, 100),
            "potencia3": random.randint(0, 100),
            "potencia4": random.randint(0, 100),
            "potencia5": random.randint(0, 100),
            "potencia6": random.randint(0, 100),
            "potencia7": random.randint(0, 100),
            "potencia8": random.randint(0, 100)
        })

        # Red
        db.Red.insert_one({
            "placa1": random.choice([True, False]),
            "placa2": random.choice([True, False]),
            "placa3": random.choice([True, False]),
            "placa4": random.choice([True, False]),
            "placa5": random.choice([True, False]),
            "placa6": random.choice([True, False]),
            "placa7": random.choice([True, False]),
            "placa8": random.choice([True, False])
        })

        # Colecciones separadas para cada semilla y plantula
        for i in range(1, 7):
            db[f"Laboratorio_s{i}"].insert_one(crear_documento())
            db[f"Laboratorio_p{i}"].insert_one(crear_documento())

        # Esperar x antes de la próxima inserción
        time.sleep(20)

if __name__ == "__main__":
    insertar_datos_aleatorios()
