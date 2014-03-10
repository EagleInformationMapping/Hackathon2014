from django.conf.urls import patterns, include, url
from vote_app.views import reply_to_sms_messages

# Uncomment the next two lines to enable the admin:
# from django.contrib import admin
# admin.autodiscover()

from vote_app.views import MobileApp

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


    url(r'^sms/$', reply_to_sms_messages),
    # , {
    #     'text': 'Hello, world!'
    # }),

    url(r'^', MobileApp.as_view()),
)
