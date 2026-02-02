import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsBuilderComponent } from './forms-builder.component';

describe('FormsBuilderComponent', () => {
  let component: FormsBuilderComponent;
  let fixture: ComponentFixture<FormsBuilderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsBuilderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FormsBuilderComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
