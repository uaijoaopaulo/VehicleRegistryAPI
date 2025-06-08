db = db.getSiblingDB("vehicleregistry");

db.createCollection("users");

db.users.insertMany([
    {
        _id: "admin@acme.com",
        password: "admin123",
        roles: ["vehicle-read", "vehicle-admin"]
    },
    {
        _id: "analista@acme.com",
        password: "analista123",
        roles: ["vehicle-read"]
    }
]);