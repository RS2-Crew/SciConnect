#!/bin/bash

echo "=== SciConnect Production Database Debug ==="
echo ""

# Check if .env file exists and has required variables
echo "1. Checking environment configuration..."
if [ -f .env ]; then
    echo "✓ .env file exists"
    
    # Check key variables without showing values
    if grep -q "ENTITY_CONNECTION_STRING" .env; then
        echo "✓ ENTITY_CONNECTION_STRING is set"
    else
        echo "✗ ENTITY_CONNECTION_STRING is missing"
    fi
    
    if grep -q "DB_SA_PASSWORD" .env; then
        echo "✓ DB_SA_PASSWORD is set"
    else
        echo "✗ DB_SA_PASSWORD is missing"
    fi
else
    echo "✗ .env file missing"
fi

echo ""
echo "2. Checking service status..."
docker-compose -f docker-compose.prod.yml ps

echo ""
echo "3. Checking DB API service logs for startup and seeding..."
echo "--- Last 30 lines from DB API startup ---"
docker logs mainservice --tail 30

echo ""
echo "4. Testing database connection..."
echo "--- Checking EntityDb database ---"
docker exec -i mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${DB_SA_PASSWORD}" << EOF
USE EntityDb;
SELECT name FROM sys.tables;
GO
EOF

echo ""
echo "5. Checking table row counts..."
docker exec -i mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${DB_SA_PASSWORD}" << EOF
USE EntityDb;
SELECT 
    'Institutions' as TableName, COUNT(*) as RowCount FROM Institutions
UNION ALL
SELECT 
    'Instruments' as TableName, COUNT(*) as RowCount FROM Instruments
UNION ALL
SELECT 
    'Microorganisms' as TableName, COUNT(*) as RowCount FROM Microorganisms
UNION ALL
SELECT 
    'Employees' as TableName, COUNT(*) as RowCount FROM Employees
UNION ALL
SELECT 
    'Keywords' as TableName, COUNT(*) as RowCount FROM Keywords
UNION ALL
SELECT 
    'Analyses' as TableName, COUNT(*) as RowCount FROM Analyses;
GO
EOF

echo ""
echo "=== Debug Complete ==="
echo ""
echo "If tables are empty, the issue is likely:"
echo "1. Seeding failed during startup"
echo "2. Database connection issues during seeding"
echo "3. Environment variables not loaded properly"
echo ""
echo "To restart the DB API service:"
echo "  docker-compose -f docker-compose.prod.yml restart db.api"