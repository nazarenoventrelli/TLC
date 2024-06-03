import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-taxi-table',
  templateUrl: './taxi-table.component.html',
  styleUrls: ['./taxi-table.component.scss']
})
export class TaxiTableComponent implements OnInit {
  displayedColumns: string[] = [];
  dataSource: any[] = [];

  constructor(private dataService: DataService) { }

  ngOnInit(): void {
    this.fetchData({
      month: 1,
      year: 2024,
      taxiType: 'Yellow',
      minimumFare: 10
    });
  }

  fetchData(queryRequest: any): void {
    this.dataService.getTaxiData(queryRequest).subscribe(data => {
      if (data && data.length > 0) {
        console.log("non transformed data", data);
        this.displayedColumns = data[0];
        this.dataSource = this.transformData(data);
        console.log('Transformed data:', this.dataSource);
      } else {
        console.log('No data received from the server');
      }
    }, error => {
      console.error('Error fetching data:', error);
    });
  }

  transformData(data: any[][]): any[] {
    const headers = data[0];
    const rows = data.slice(1);
    return rows.map(row => {
      const obj: any = {};
      headers.forEach((header: string, index: number) => {
        obj[header] = row[index];
      });
      return obj;
    });
  }

  onSearch(queryRequest: any): void {
    this.fetchData(queryRequest);
  }
}
