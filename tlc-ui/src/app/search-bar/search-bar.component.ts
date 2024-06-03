import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss']
})
export class SearchBarComponent {
  @Output() search = new EventEmitter<any>();
  filterForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.filterForm = this.fb.group({
      month: [1],
      year: [2024],
      taxiType: ['Yellow'],
      minimumFare: [10]
    });
  }

  onSearch(): void {
    this.search.emit(this.filterForm.value);
  }

  isFhvSelected(): boolean {
    return this.filterForm.get('taxiType')?.value === 'Fhv';
  }
}
