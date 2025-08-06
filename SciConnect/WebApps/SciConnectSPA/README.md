# SciConnect - Medical Research Platform Frontend

A professional Angular-based frontend for the SciConnect medical research platform, designed specifically for medical professionals.

## Features

- **Professional Medical UI**: Clean, modern interface designed for medical professionals
- **Secure Authentication**: JWT-based authentication with refresh tokens
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Medical Theme**: Healthcare-focused design with medical icons and color scheme
- **Form Validation**: Comprehensive client-side validation with user-friendly error messages
- **Loading States**: Professional loading indicators and user feedback
- **Dashboard**: Post-login dashboard with quick actions and recent activity

## Prerequisites

- Node.js (v16 or higher)
- Angular CLI (v17 or higher)
- .NET Core (for backend services)

## Installation

1. Navigate to the project directory:
   ```bash
   cd SciConnect/WebApps/SciConnectSPA
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

## Running the Application

### Development Mode

1. Start the Angular development server:
   ```bash
   npm start
   ```

2. The application will be available at `http://localhost:4200`

### Backend Requirements

Make sure the backend services are running:

1. **Identity Server**: Should be running on `http://localhost:4000`
2. **Database API**: Should be running on the configured port

## Project Structure

```
src/app/
├── dashboard/                 # Dashboard component (post-login)
├── identity/                 # Authentication module
│   ├── domain/              # Domain models and services
│   ├── feature-authentification/
│   │   └── login-form/      # Login form component
│   └── identity.component.ts # Identity wrapper component
├── shared/                  # Shared services and utilities
└── app.component.ts         # Main app component
```

## Key Features

### Login Form
- Professional medical-themed design
- Real-time form validation
- Loading states and error handling
- Secure authentication flow
- Responsive design for all devices

### Dashboard
- Welcome screen with personalized greeting
- Quick action cards for common tasks
- Recent activity feed
- Professional medical icons and styling

### Authentication
- JWT token management
- Automatic token refresh
- Secure logout functionality
- Error handling for network issues

## Styling

The application uses:
- **Font Awesome**: For medical and UI icons
- **Inter Font**: Modern, readable typography
- **CSS Grid & Flexbox**: Responsive layouts
- **CSS Custom Properties**: Consistent theming
- **Medical Color Palette**: Professional healthcare colors

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Development

### Adding New Features

1. Create components in the appropriate module
2. Update routing configuration
3. Add necessary services to the domain layer
4. Implement responsive design
5. Add proper error handling

### Styling Guidelines

- Use the established color palette
- Maintain medical theme consistency
- Ensure accessibility compliance
- Test on multiple screen sizes

## Troubleshooting

### Common Issues

1. **CORS Errors**: Ensure backend CORS is configured for `http://localhost:4200`
2. **Authentication Failures**: Verify backend is running on port 4000
3. **Build Errors**: Clear node_modules and reinstall dependencies

### Development Tips

- Use Angular DevTools for debugging
- Monitor network requests in browser dev tools
- Check console for authentication errors
- Test on different devices and browsers

## Contributing

1. Follow the established code structure
2. Maintain medical theme consistency
3. Add proper error handling
4. Test on multiple devices
5. Update documentation as needed

## License

This project is part of the SciConnect medical research platform.
