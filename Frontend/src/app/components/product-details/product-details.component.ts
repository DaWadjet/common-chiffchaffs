import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit {
  constructor() {}

  isAdmin = true;
  isLoading = false;
  productName = 'A Book';
  imageUrl =
    'https://localhost:5001/previews/cd2755ea-10a5-4815-977f-1cb90553bcc4.bmp';
  description = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.';
  price = 132.99;
  uploadedAt = new Date();
  comments = [
    {
      content:
        'blablabla ez egy hosszzabgb komment most mi lesz ur istenem nehogy kilogjon mert akkor aztan vege mindennek',
      commenterName: 'bonjour',
    },
    { content: 'trololo', commenterName: 'egyik' },
  ];

  ngOnInit(): void {}
}
