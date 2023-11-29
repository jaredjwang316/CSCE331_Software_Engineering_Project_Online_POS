export class Cart {
    items = [Item];
    total = 0;

    Length() {
        var count = 0;
        this.items.forEach(function(item) {
            count += item.quantity;
        });
        return count;
    }
}

export class Item {
    product;
    options = [];
    quantity = 0;
    cost = 0;

    // method to calculate cost
    TotalPrice() {
        var total = 0.0;
        total += this.product.price;
        this.options.forEach(function(option) {
            total += option.price;
        });
        total *= this.quantity;
        return total;
    }
}
