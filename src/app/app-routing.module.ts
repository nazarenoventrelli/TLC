import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaxiTableComponent } from './taxi-table/taxi-table.component';

const routes: Routes = [
  { path: '', component: TaxiTableComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
