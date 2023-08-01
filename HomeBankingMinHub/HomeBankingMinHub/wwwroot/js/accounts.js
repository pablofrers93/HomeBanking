var app = new Vue({
    el:"#app",
    data:{
        clientInfo: {},
        loans: [],
        accounts: [],
        error: null
    },
    methods:{
        getData: function(){
            axios.get("/api/clients/10002")
            .then(function (response) {
                //get client ifo
                app.accounts = response.data.accounts.$values;
                app.loans = response.data.loans.$values;
                console.log(app.accounts)
                app.clientInfo = response.data;
            })
            .catch(function (error) {
                // handle error
                app.error = error;
            })
        },
        formatDate: function(date){
            return new Date(date).toLocaleDateString('en-gb');
        }
    },
    mounted: function(){
        this.getData();
    }
})