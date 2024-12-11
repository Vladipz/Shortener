import { Component } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { NgIf } from '@angular/common';
import { Subscription } from 'rxjs';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [NgIf, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  email: string | null = null;
  private subscription: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {
    this.subscription = this.authService.userEmail$.subscribe((email) => {
      this.email = email;
    });
  }
  login() {
    this.router.navigate(['/login']);
  }

  logout() {
    this.authService.logout();
  }

  ngOnInit() {
    const isLogged = this.authService.isAuth;
    console.log('Is logged:', isLogged);
    if (isLogged) {
      this.email = this.authService.getUserEmail();
    }
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
