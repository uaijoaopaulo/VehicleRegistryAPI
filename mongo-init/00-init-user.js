db = db.getSiblingDB("vehicleregistry");

db.createUser({
    user: 'usrVehicleRegistry',
    pwd: '8xNpyq8dtv',
    roles: [
        { role: 'readWrite', db: 'vehicleregistry' },
        { role: 'dbAdmin', db: 'vehicleregistry' }
    ]
});