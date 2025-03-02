import { CommonModule, NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Meetali';

  //constructor(private httpClient : HttpClient){} --> old ways to DI
  http = inject(HttpClient); //---> new way to DI
  users: any;

  ngOnInit(): void {
    this.http.get('http://localhost:5062/api/users').subscribe( {
      next: response => this.users = response,
      error: err => console.log(err),
      complete: () => console.log('Request has complete')
  })
}
  
}
