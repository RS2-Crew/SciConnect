import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

export interface FilterPanelItem {
  id: number;
  name?: string;
  firstName?: string;
  lastName?: string;
  [key: string]: any;
}

export interface FilterPanelConfig {
  title: string;
  icon: string;
  allowMultiple: boolean;
  showCheckboxes: boolean;
  showAddButton: boolean;
  addButtonColor: 'green' | 'red';
}

@Component({
  selector: 'app-filter-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="filter-panel" [class.expanded]="isExpanded">
      <div class="panel-header" (click)="toggleExpanded()">
        <div class="header-content">
          <span class="panel-title">{{ config.title }}</span>
          <div class="header-actions">
            <button 
              *ngIf="config.showAddButton" 
              class="btn-add" 
              [class.green]="config.addButtonColor === 'green'"
              [class.red]="config.addButtonColor === 'red'"
              (click)="onAddClick($event)">
              <i class="fas fa-plus"></i>
            </button>
            <button class="btn-toggle" (click)="toggleExpanded()">
              <i class="fas" [class.fa-chevron-down]="isExpanded" [class.fa-chevron-left]="!isExpanded"></i>
            </button>
          </div>
        </div>
      </div>
      
      <div class="panel-content" *ngIf="isExpanded">
        <div class="loading-spinner" *ngIf="isLoading">
          <i class="fas fa-spinner fa-spin"></i>
        </div>
        
        <div class="items-list" *ngIf="!isLoading">
          <div 
            *ngFor="let item of items" 
            class="item"
            [class.selected]="isItemSelected(item)"
            (click)="onItemClick(item)">
            
            <div class="item-content">
              <div class="checkbox-container" *ngIf="config.showCheckboxes">
                <input 
                  type="checkbox" 
                  [checked]="isItemSelected(item)"
                  (change)="onCheckboxChange(item, $event)"
                  (click)="$event.stopPropagation()">
              </div>
                             <span class="item-name">{{ getDisplayName(item) }}</span>
            </div>
          </div>
          
          <div class="no-items" *ngIf="items.length === 0">
            <p>No items available</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./filter-panel.component.css']
})
export class FilterPanelComponent implements OnInit, OnDestroy {
  @Input() config!: FilterPanelConfig;
  @Input() items: FilterPanelItem[] = [];
  @Input() selectedItems: FilterPanelItem[] = [];
  @Input() isLoading = false;
  @Input() isExpanded = true;

  @Output() itemSelected = new EventEmitter<FilterPanelItem>();
  @Output() itemDeselected = new EventEmitter<FilterPanelItem>();
  @Output() addClicked = new EventEmitter<void>();
  @Output() expandedChanged = new EventEmitter<boolean>();

  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    // Component initialization
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggleExpanded(): void {
    this.isExpanded = !this.isExpanded;
    this.expandedChanged.emit(this.isExpanded);
  }

  onItemClick(item: FilterPanelItem): void {
    if (this.config.allowMultiple) {
      if (this.isItemSelected(item)) {
        this.onItemDeselected(item);
      } else {
        this.onItemSelected(item);
      }
    } else {
      // Single selection - replace current selection
      this.selectedItems = [item];
      this.itemSelected.emit(item);
    }
  }

  onItemSelected(item: FilterPanelItem): void {
    if (!this.isItemSelected(item)) {
      this.selectedItems = [...this.selectedItems, item];
      this.itemSelected.emit(item);
    }
  }

  onItemDeselected(item: FilterPanelItem): void {
    this.selectedItems = this.selectedItems.filter(selected => selected.id !== item.id);
    this.itemDeselected.emit(item);
  }

  onCheckboxChange(item: FilterPanelItem, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      this.onItemSelected(item);
    } else {
      this.onItemDeselected(item);
    }
  }

  onAddClick(event: Event): void {
    event.stopPropagation();
    this.addClicked.emit();
  }

  isItemSelected(item: FilterPanelItem): boolean {
    return this.selectedItems.some(selected => selected.id === item.id);
  }

  getDisplayName(item: FilterPanelItem): string {
    if (item.name) {
      return item.name;
    }
    if (item.firstName && item.lastName) {
      return `${item.firstName} ${item.lastName}`;
    }
    if (item.firstName) {
      return item.firstName;
    }
    if (item.lastName) {
      return item.lastName;
    }
    return 'Unknown';
  }
}
