import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaxiTableComponent } from './TaxiTableComponent';

describe('TaxiTableComponent', () => {
  let component: TaxiTableComponent;
  let fixture: ComponentFixture<TaxiTableComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TaxiTableComponent]
    });
    fixture = TestBed.createComponent(TaxiTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
