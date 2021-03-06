from django.conf.urls import patterns, include, url
from vote_app.views import reply_to_sms_messages
from vote_app.views import jamie_view
from vote_app.views import send_text

# Uncomment the next two lines to enable the admin:
# from django.contrib import admin
# admin.autodiscover()

from vote_app.views import MobileApp
from vote_app.views import proxy_view
from vote_app.views import proxy_view_gp
from vote_app.views import load_test
from vote_app.views import generate_token_view

urlpatterns = patterns('',
    # Examples:
    # url(r'^$', 'SAMVotes.views.home', name='home'),
    # url(r'^SAMVotes/', include('SAMVotes.foo.urls')),

    # Uncomment the admin/doc line below to enable admin documentation:
    # url(r'^admin/doc/', include('django.contrib.admindocs.urls')),

    # Uncomment the next line to enable the admin:
    # url(r'^admin/', include(admin.site.urls)),


    # ...
    # url(r'^sms/$', 'django_twilio.views.sms', {
    #     'message': 'Thanks for the SMS. Talk to you soon!',
    # }),
    # ...

    url(r'^$', MobileApp.as_view()),
    url(r'^sms', reply_to_sms_messages),
    url(r'^jamie', jamie_view),
    url(r'^send_text', send_text),
    url(r'^proxy/(?P<path>.*)', proxy_view),
    url(r'^proxy_gp/(?P<path>.*)', proxy_view_gp),
    url(r'^loaderio-5aae65f5b1da419918b14ce0624c494d', load_test),
    url(r'^generate_token', generate_token_view),
)
