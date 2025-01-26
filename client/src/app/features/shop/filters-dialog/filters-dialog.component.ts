import { Component, inject } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { MatDivider } from '@angular/material/divider';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filters-dialog',
  imports: [
    MatSelectionList,
    MatListOption,
    MatDivider,
    MatButton,
    FormsModule
  ],
  templateUrl: './filters-dialog.component.html',
  styleUrl: './filters-dialog.component.scss'
})
export class FiltersDialogComponent {
  shopService = inject(ShopService);
  private dialogRef = inject(MatDialogRef);
  data = inject(MAT_DIALOG_DATA);

  selectedBrands : string[] = this.data.selectedBrands;
  selectedTypes : string[] = this.data.selectedTypes;

  applyFilters(){
    this.dialogRef.close({
      selectedBrands : this.selectedBrands,
      selectedTypes : this.selectedTypes
    })
  }

}
