relmah = (function () {
    "use strict";

    return function (endpoint, subs) {
        var conn,
            errors = new Rx.Subject(),
            applications = new Rx.Subject(),
            es, as,
            apps = {};

        subs = subs || {};

        return {
            start: function (opts) {
                conn = $.hubConnection(endpoint);

                var proxy = conn.createHubProxy('relmah-errors');

                proxy.on('error', function (p) {
                    errors.onNext(p);
                });
                proxy.on('applications', function (existingApps, removedApps) {
                    var i, k;
                    for (i = 0; i < existingApps.length; i++) {
                        k = existingApps[i];
                        !apps[k] && applications.onNext({ name: k, action: 'added' });
                        apps[k] = true;
                    }
                    for (i = 0; removedApps && i < removedApps.length; i++) {
                        k = removedApps[i];
                        apps[k] && applications.onNext({ name: k, action: 'removed' });
                        apps[k] = false;
                    }
                });

                conn.qs = { user: opts && opts.user };

                return conn.start();
            },
            getErrors: function () {
                es = es || (subs.errors && typeof (subs.errors) === 'function' && subs.errors(errors) || errors);
                return es;
            },
            getApplications: function () {
                as = as || (subs.applications && typeof (subs.applications) === 'function' && subs.applications(applications) || applications);
                return as;
            },
            stop: function () {
                apps = {};
                return conn && conn.stop();
            },
            disconnected: function(callback) {
                return conn.disconnected(callback);
            }
        };
    };
})();