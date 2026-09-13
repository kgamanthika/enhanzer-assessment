import {
    Component,
    OnInit,
    inject
} from '@angular/core';

import {
    CommonModule
} from '@angular/common';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import {
    finalize
} from 'rxjs';

import {
    Router
} from '@angular/router';

import {
    Location,
    PurchaseBillItem
} from '../../models/models';

import {
    LocationService
} from '../../services/location.service';

import {
    PurchaseBillService
} from '../../services/purchase-bill.service';

import {
    AuthService
} from '../../services/auth.service';

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './purchase-bill.component.html',
  styleUrl: './purchase-bill.component.scss'
})
export class PurchaseBillComponent
  implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly locationService =
    inject(LocationService);
  private readonly purchaseBillService =
    inject(PurchaseBillService);
  private readonly authService =
    inject(AuthService);
  private readonly router = inject(Router);

  readonly items = [
    'Mango',
    'Apple',
    'Banana',
    'Orange',
    'Grapes',
    'Kiwi',
    'Strawberry'
  ];

  locations: Location[] = [];

  billItems: PurchaseBillItem[] = [];

  loadingLocations = false;
  submitting = false;

  errorMessage = '';
  successMessage = '';

  billForm = this.fb.nonNullable.group({
    itemName: [
      '',
      Validators.required
    ],

    batchName: [
      '',
      Validators.required
    ],

    standardCost: [
      0,
      [
        Validators.required,
        Validators.min(0.01)
      ]
    ],

    standardPrice: [
      0,
      [
        Validators.required,
        Validators.min(0.01)
      ]
    ],

    quantity: [
      1,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    discountPercentage: [
      0,
      [
        Validators.required,
        Validators.min(0),
        Validators.max(100)
      ]
    ]
  });

  ngOnInit(): void {
    this.loadLocations();
  }

  loadLocations(): void {
    this.loadingLocations = true;

    this.locationService
      .getLocations()
      .pipe(
        finalize(() => {
          this.loadingLocations = false;
        })
      )
      .subscribe({
        next: locations => {
          this.locations = locations;
        },
        error: error => {
          if (error.status === 401) {
            this.authService.logout();
            this.router.navigate(['/login']);
            return;
          }

          this.errorMessage =
            'Unable to load locations.';
        }
      });
  }

  get totalItems(): number {
    return this.billItems.length;
  }

  get totalQuantity(): number {
    return this.billItems.reduce(
      (sum, item) => sum + item.quantity,
      0
    );
  }

  get totalCost(): number {
    return this.billItems.reduce(
      (sum, item) => sum + item.totalCost,
      0
    );
  }

  get totalSelling(): number {
    return this.billItems.reduce(
      (sum, item) => sum + item.totalSelling,
      0
    );
  }

  calculateTotalCost(): number {
    const value = this.billForm.getRawValue();

    const gross =
      value.standardCost * value.quantity;

    const discount =
      gross *
      (value.discountPercentage / 100);

    return gross - discount;
  }

  calculateTotalSelling(): number {
    const value = this.billForm.getRawValue();

    return value.standardPrice * value.quantity;
  }

  addItem(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.billForm.invalid) {
      this.billForm.markAllAsTouched();
      return;
    }

    const value = this.billForm.getRawValue();

    const item: PurchaseBillItem = {
      itemName: value.itemName,
      batchName: value.batchName,
      standardCost: value.standardCost,
      standardPrice: value.standardPrice,
      quantity: value.quantity,
      discountPercentage:
        value.discountPercentage,
      totalCost: this.calculateTotalCost(),
      totalSelling: this.calculateTotalSelling()
    };

    this.submitting = true;

    this.purchaseBillService
      .create(item)
      .pipe(
        finalize(() => {
          this.submitting = false;
        })
      )
      .subscribe({
        next: () => {
          this.billItems.push(item);

          this.successMessage =
            'Item added successfully.';

          this.billForm.reset({
            itemName: '',
            batchName: '',
            standardCost: 0,
            standardPrice: 0,
            quantity: 1,
            discountPercentage: 0
          });
        },

        error: error => {
          if (error.status === 401) {
            this.authService.logout();
            this.router.navigate(['/login']);
            return;
          }

          this.errorMessage =
            error.error?.message ??
            'Unable to add purchase bill item.';
        }
      });
  }

  removeItem(index: number): void {
    this.billItems.splice(index, 1);
  }

  logout(): void {
    this.authService.logout();

    this.router.navigate(['/login']);
  }
}