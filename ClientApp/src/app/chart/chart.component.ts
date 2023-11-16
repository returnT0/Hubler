import {AfterViewInit, Component, ElementRef} from '@angular/core';
import {AuthenticationService} from "../service/auth-service/authentication.service";

declare var ApexCharts: any;

interface Product {
    imageUrl: string;
    id: number;
    title: string;
    date?: string;
    soldQuantity?: number;
}

@Component({
    selector: 'app-chart',
    templateUrl: './chart.component.html',
    styleUrls: ['./chart.component.css']
})
export class ChartComponent implements AfterViewInit {
    selectedValue: string = 'Last 7 days';
    isDropdownVisible: boolean = false;
    salesValue: string = '12,423';
    percentageChange: number = 23;
    topProducts: Product[] = [
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-3.jpg',
            id: 1,
            title: 'Product 1',
            soldQuantity: 100
        },
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-5.jpg',
            id: 2,
            title: 'Product 2',
            soldQuantity: 100
        },
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-6.jpg',
            id: 3,
            title: 'Product 3',
            soldQuantity: 100
        },
    ];
    expiredProducts: Product[] = [
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-6.jpg',
            id: 1,
            title: 'Fresh Peaches',
            date: '2021/02/01'
        },
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-3.jpg',
            id: 2,
            title: 'Corn Onion Muffaleta Bread',
            date: '2021/02/01'
        },
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-3.jpg',
            id: 3,
            title: 'Fresh Blueberries',
            date: '2021/02/03'
        },
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-3.jpg',
            id: 4,
            title: 'Fresh Blueberries',
            date: '2021/02/03'
        },
        {
            imageUrl: 'https://flowbite.s3.amazonaws.com/docs/gallery/masonry/image-3.jpg',
            id: 5,
            title: 'Fresh Blueberries',
            date: '2021/02/03'
        },
    ];
    private options: any;

    constructor(private elRef: ElementRef, private authService: AuthenticationService) {
        this.options = {
            series: [
                {
                    name: "Developer Edition",
                    data: [1500, 1418, 1456, 1526, 1356, 1256],
                    color: "#f2cd3b",
                },
                {
                    name: "Designer Edition",
                    data: [643, 413, 765, 412, 1423, 1731],
                    color: "#db1aae",
                },
            ],
            chart: {
                height: "100%",
                maxWidth: "100%",
                type: "area",
                fontFamily: "Inter, sans-serif",
                dropShadow: {
                    enabled: false,
                },
                toolbar: {
                    show: false,
                },
            },
            tooltip: {
                enabled: true,
                x: {
                    show: false,
                },
            },
            legend: {
                show: false
            },
            fill: {
                type: "gradient",
                gradient: {
                    opacityFrom: 0.55,
                    opacityTo: 0,
                    shade: "#f2cd3b",
                    gradientToColors: ["#db1aae"],
                },
            },
            dataLabels: {
                enabled: false,
            },
            stroke: {
                width: 6,
            },
            grid: {
                show: false,
                strokeDashArray: 4,
                padding: {
                    left: 2,
                    right: 2,
                    top: 0
                },
            },
            xaxis: {
                categories: ['01 February', '02 February', '03 February', '04 February', '05 February', '06 February', '07 February'],
                labels: {
                    show: false,
                },
                axisBorder: {
                    show: false,
                },
                axisTicks: {
                    show: false,
                },
            },
            yaxis: {
                show: false,
                labels: {
                    formatter: (value: number) => '$' + value
                }
            },
        };
    }

    toggleDropdown(): void {
        this.isDropdownVisible = !this.isDropdownVisible;
    }

    updateValue(value: string): void {
        this.selectedValue = value;
        this.isDropdownVisible = false;
    }

    ngAfterViewInit(): void {
        this.initChart();
    }

    public isUserAuthenticated(): boolean {
        return this.authService.isUserSignedIn();
    }

    private initChart(): void {
        const chartContainer = this.elRef.nativeElement.querySelector('#data-series-chart');
        if (chartContainer && ApexCharts) {
            const chart = new ApexCharts(chartContainer, this.options);
            chart.render();
        }
    }
}