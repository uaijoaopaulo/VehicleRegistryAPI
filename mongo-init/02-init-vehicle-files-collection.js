db = db.getSiblingDB("vehicleregistry");

db.createCollection("vehicle-files");

db.getCollection("vehicle-files").createIndex(
    { "generatedAt": 1 },
    { expireAfterSeconds: 86400 }
);