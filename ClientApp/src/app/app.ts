import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  private moveHandler?: (e: MouseEvent) => void;

  ngOnInit(): void {
    const layer = document.querySelector('.cursor-layer');
    if (!layer) return;

    this.moveHandler = (e: MouseEvent) => {
      const spark = document.createElement('span');
      spark.className = 'cursor-spark';
      spark.style.left = `${e.clientX}px`;
      spark.style.top = `${e.clientY}px`;
      layer.appendChild(spark);
      window.setTimeout(() => spark.remove(), 700);
    };
    window.addEventListener('mousemove', this.moveHandler);
  }

  ngOnDestroy(): void {
    if (this.moveHandler) {
      window.removeEventListener('mousemove', this.moveHandler);
    }
  }
}
