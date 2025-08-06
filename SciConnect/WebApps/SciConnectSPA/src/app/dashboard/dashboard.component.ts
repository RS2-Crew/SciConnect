import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  public userName: string = 'Medical Professional';
  public currentDate: Date = new Date();

  constructor(private router: Router) {}

  ngOnInit(): void {
    // In a real app, you would get the user name from the authentication service
    // this.userName = this.authService.getCurrentUser()?.name || 'Medical Professional';
  }

  public onLogout(): void {
    // In a real app, you would call the logout service
    this.router.navigate(['/identity/login']);
  }

  public getGreeting(): string {
    const hour = this.currentDate.getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }
} 