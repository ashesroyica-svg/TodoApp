import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

/** Manages the global loading state for the spinner overlay. */
@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly _loading = new BehaviorSubject<boolean>(false);

  /** Observable that emits the current loading state. */
  readonly loading$ = this._loading.asObservable();

  /** Shows the global loading spinner. */
  show(): void {
    this._loading.next(true);
  }

  /** Hides the global loading spinner. */
  hide(): void {
    this._loading.next(false);
  }
}
