var app = new Vue({
    el:"#app",
    data:{
        accountInfo: {},
        //error: null,
        errorToats: null,
        errorMsg: null,
    },
    methods:{
        getData: function () {
            console.log("entrando en el metodo getData")
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get('id');
            axios.get(`/api/accounts/${id}`)
            .then(function (response) {
                //get client ifo
                console.log("dentro de function response..")
                app.accountInfo = response.data;
                app.accountInfo.transactions.$values.sort((a,b) => parseInt(b.id - a.id))
            })
            .catch(function (error) {
                // handle error
                //app.error = error;
                this.errorMsg = "Error getting data";
                this.errorToats.show();
            })
        },
        formatDate: function(date){
            return new Date(date).toLocaleDateString('en-gb');
        },
        signOut: function () {
            axios.post('/api/auth/logout')
                .then(response => window.location.href = "/index.html")
                .catch(() => {
                    this.errorMsg = "Sign out failed"
                    this.errorToats.show();
                })
        },
    },
    mounted: function () {
        this.errorToats = new bootstrap.Toast(document.getElementById('danger-toast'));
        this.getData();
    }
})

