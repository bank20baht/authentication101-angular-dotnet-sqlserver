import { LoaderComponent } from '@/shared';
import {
  ApplicationRef,
  ComponentRef,
  createComponent,
  EnvironmentInjector,
  Injectable,
} from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  private loaderRef: ComponentRef<LoaderComponent> | null = null;
  private showCount = 0;

  constructor(
    private appRef: ApplicationRef,
    private injector: EnvironmentInjector,
  ) {}

  show(): void {
    this.showCount++;

    if (this.loaderRef) return;

    const compRef = createComponent(LoaderComponent, {
      environmentInjector: this.injector,
    });

    this.appRef.attachView(compRef.hostView);
    const domElement = (compRef.hostView as any).rootNodes[0] as HTMLElement;

    const overlayContainer = document.createElement('div');
    overlayContainer.className = 'loading-overlay-container';
    overlayContainer.style.cssText = `
      position: fixed;
      inset: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 999999;
    `;
    overlayContainer.appendChild(domElement);
    document.body.appendChild(overlayContainer);

    this.loaderRef = compRef;
  }

  hide(force = false): void {
    if (force) {
      this.showCount = 0;
    } else {
      this.showCount = Math.max(0, this.showCount - 1);
    }

    if (this.showCount === 0 && this.loaderRef) {
      this.appRef.detachView(this.loaderRef.hostView);
      this.loaderRef.destroy();
      this.loaderRef = null;

      const overlayContainer = document.querySelector('.loading-overlay-container');
      if (overlayContainer) overlayContainer.remove();
    }
  }
}
