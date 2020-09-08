const Mixin = {
    methods: {
        formatDate(date) {
            let d;
            // see if date is coming from server
            date === undefined
                ? d = new Date()
                : d = new Date(Date.parse(date)); // date from server
            let _day = d.getDate();
            let _month = d.getMonth() + 1;
            let _year = d.getFullYear();
            let _hour = d.getHours();
            let _min = d.getMinutes();
            if (_min < 10) { _min = "0" + _min; }
            return _year + "-" + _month + "-" + _day + " " + _hour + ":" + _min;
        }
    }
};
// register modal component
Vue.component("modal", {
    template: "#modal-template",
    props: {
        order: {},
        details: {}
    },
    mixins: [Mixin]
});
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
        async loadAndShowModal() {
            try {
                this.modalStatus = "Loading Details..";
                this.showModal = true;
                let response = await fetch(`/GetOrderDetails/${this.orderForModal.id}`);
                this.detailsForModal = await response.json();
                this.modalStatus = "";
            } catch (error) {
                console.log(error.statusText);
            }
        },


    },
    mixins: [Mixin],
    mounted() { this.getOrders(); },
    data: {
        orders: [],
        showModal: false,
        orderForModal: {},
        detailsForModal: {},
        status: "",
        modalStatus: ""
    }



});

Vue.filter('toCurrency', function (value) {
    if (typeof value !== "number") {
        return value;
    }
    const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 0
    });
    return formatter.format(value);
});






