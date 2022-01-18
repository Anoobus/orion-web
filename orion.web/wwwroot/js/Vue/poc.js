// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const genPOC = function vuePOCApp(targetId) {
    //console.log(`${name}, you are awesome!`);
    var app2 = new Vue({
        el: '#vue-app',
        data() {
            return {
                isLoaded: true,
                info: [],
                test: {
                    out: ""
                }
            }
        },
        templa
        created() {
            this.debouncedHandler = _.debounce(event => {
                console.log("new value: ", event.target.value);
            }, 500);
        },
        mounted() {
            //.get('https://localhost:5001/orion-api/v1/expenditures/arc-flash-labels/week/{week-id:int}/employee/{employee-id:Guid}')
            axios
                .get('https://localhost:5001/orion-api/v1/jobs')
                .then(response => {
                    this.info = _.take(response.data.data, 2);
                    isLoaded = true;
                });

        },
        beforeUnmount() {
            this.debouncedHandler.cancel();
        },
        methods: {
            save: function (event) {
                console.log("saved called", event.target);
            }
        }
    })

};
