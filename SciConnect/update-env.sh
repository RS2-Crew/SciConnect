#!/bin/bash

# Script to update .env file with JWT configuration
echo "Updating .env file with JWT configuration..."

# Create .env file from .env.example if it doesn't exist
if [ ! -f .env ]; then
    echo "Creating .env file from .env.example..."
    cp .env.example .env
    echo ".env file created successfully!"
else
    echo ".env file already exists, updating..."
fi

# Update the .env file to add JWT configuration if not present
if ! grep -q "JWT_VALID_ISSUER" .env; then
    echo "" >> .env
    echo "# JWT Configuration" >> .env
    echo "JWT_VALID_ISSUER=SciConnect Identity" >> .env
    echo "JWT_VALID_AUDIENCE=SciConnect" >> .env
    echo "JWT_SECRET_KEY=MyVeryLongLongSecretMessage256bi" >> .env
    echo "JWT_EXPIRES=15" >> .env
    echo "" >> .env
    echo "# Refresh Token Configuration" >> .env
    echo "REFRESH_TOKEN_EXPIRES=30" >> .env
    echo "" >> .env
    echo "# PM Settings" >> .env
    echo "PM_REGISTRATION_PASSWORD=AQAAAAIAAYagAAAAEEqS4gpMsJwJAwMMr09fKAgRsOtwx5bItcfMsZ1FJcqyWkWfIH/r54ebYqA0sbElsw==" >> .env
    
    echo "JWT configuration added to .env file!"
else
    echo "JWT configuration already exists in .env file."
fi

echo "Environment file updated successfully!"
echo ""
echo "Next steps:"
echo "1. Review the .env file and adjust values if needed"
echo "2. Run: docker-compose -f docker-compose.prod.yml down"
echo "3. Run: docker-compose -f docker-compose.prod.yml up -d --build"