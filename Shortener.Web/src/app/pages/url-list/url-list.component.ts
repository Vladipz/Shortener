import { Component, inject } from '@angular/core';
import { UrlService } from '../../data/services/url.service';
import { UrlData } from '../../data/interfaces/url-data.interface';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { CreateUrlComponent } from '../../common-ui/create-url/create-url.component';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-url-list',
  standalone: true,
  imports: [NgIf, NgFor, DatePipe, CreateUrlComponent],
  templateUrl: './url-list.component.html',
  styleUrl: './url-list.component.scss',
})
export class UrlListComponent {
  // Типізуємо масив з об'єктами UrlData
  urls: UrlData[] = [];
  isAuth: boolean = false;

  urlService = inject(UrlService);
  authService = inject(AuthService);

  ngOnInit(): void {
    this.loadUrls();
    this.authService.isAuthenticated$.subscribe((isAuth) => {
      this.isAuth = isAuth;
    });
  }

  loadUrls(): void {
    this.urlService.getUrls().subscribe(
      (data) => {
        this.urls = data;
      },
      (error) => {
        console.error('Error loading URLs', error);
      },
    );
  }

  // This method will be called when the child emits an event
  onNewUrlCreated(newUrl: UrlData): void {
    this.urls.push(newUrl); // Add the new URL to the list
  }

  ngOnDestroy(): void {
    console.log('UrlListComponent destroyed');
  }
}
