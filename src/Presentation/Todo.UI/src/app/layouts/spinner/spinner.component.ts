import { Component, inject } from '@angular/core';
import { AsyncPipe, NgIf } from '@angular/common';
import { LoadingService } from '../../core/services/loading.service';

/** Full-screen overlay spinner shown during active HTTP requests. */
@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [NgIf, AsyncPipe],
  templateUrl: './spinner.component.html',
  styleUrl: './spinner.component.scss'
})
export class SpinnerComponent {
  readonly loadingService = inject(LoadingService);
}
