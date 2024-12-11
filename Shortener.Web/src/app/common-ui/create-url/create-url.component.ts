import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UrlService } from '../../data/services/url.service';
import { UrlData } from '../../data/interfaces/url-data.interface';

@Component({
  selector: 'app-create-url',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-url.component.html',
  styleUrl: './create-url.component.scss',
})
export class CreateUrlComponent implements OnInit {
  @Output() newUrlCreated = new EventEmitter<UrlData>(); // EventEmitter to send data to parent
  urlForm!: FormGroup;
  shortenedUrl: string | null = null;
  urlService = inject(UrlService);

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.urlForm = this.fb.group({
      originalUrl: [
        '',
        [Validators.required, Validators.pattern('https?://.+')],
      ],
    });
  }

onSubmit(): void {
    if (this.urlForm.valid) {
      const longUrl = this.urlForm.get('originalUrl')?.value;
      console.log('Creating short URL for:', longUrl);


      this.urlService.createUrl(longUrl).subscribe(
        (newUrl) => {
          // Emit the new URL to the parent component
          console.log('New URL created:', newUrl);
          this.newUrlCreated.emit(newUrl);
          this.urlForm.reset();
        },
        (error) => {
          console.error('Error creating short URL', error);
        }
      );
    }
  }
}
