import { Component } from '@angular/core';
import { Router, NavigationEnd, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [RouterOutlet, CommonModule],
  standalone: true
})
export class AppComponent {
  public showHeader: boolean = true;

  constructor(private router: Router) {
    // Hide header on login/register pages
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event) => {
      const navigationEvent = event as NavigationEnd;
      const url = navigationEvent.url;
      this.showHeader = !url.includes('/identity/login') && !url.includes('/identity/register');
    });
  }
}
