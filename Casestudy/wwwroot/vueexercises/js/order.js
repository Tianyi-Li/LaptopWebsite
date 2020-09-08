const orderlist = new Vue({
    el: '#orders',
    methods: {
        async getOrders() {
            try {
                this.status = 'Loading... ';
                let response = await fetch(`/GetOrders`);
                if (!response.ok) // or check for response.status
                    throw new Error(`Status - ${response.status}, Text - ${response.statusText}`);
                let data = await response.json(); // this returns a promise, so we await it
                this.orders = data;
                //for (var i = 0; i < this.trays.length; i++) {
                //    this.trays[i].dateCreated = formatDate(this.trays[i].dateCreated);
                //}
                this.status = '';
            } catch (error) {
                this.status = error;
                console.log(error);
            }
        },
      
    },
   
    mounted() { this.getOrders(); },
    data: {
        orders: [],
        status: ""
      
    }



});






