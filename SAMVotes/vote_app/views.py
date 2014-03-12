# Create your views here.
from django.http import HttpResponse
from django.conf import settings
from django.views.generic import TemplateView
from twilio.rest import TwilioRestClient
#from models import User
import json
import datetime
from django.views.generic import TemplateView
from httplib2 import Http
import proxy.views
from django.views.decorators.http import require_http_methods
from django.views.decorators.csrf import csrf_exempt


def load_test(request):
    return HttpResponse("loaderio-5aae65f5b1da419918b14ce0624c494d")#, 'text/plain')


def generate_token_view(request):
    context = {}
    h = Http()
    resp, content = h.request("https://www.arcgis.com/sharing/oauth2/token?client_id=IWuR7oUBNlG38ytK&client_secret=012e8dc3d4854c978b11106b2e49a227&grant_type=client_credentials&expires_in=86400")
    data = json.loads(content)
    context['token_response'] = content
    context['token'] = data['access_token']
    return HttpResponse(json.dumps(context), 'application/json')


def _get_token():
    return '-VC2Pc-_UszjkuafHlzUi4j2WrYSn4F6En9BMi3bgliVRi_Effw1OUUdg8Y5wEsfmBmIiRqbMGp5H-PqIIbJB96IdBGoD8RbukiyWhMTbDIrw2bJPRElD4g_9mO-Y0tD8-m-uqAoB6hd6LsKss0naQ..'

@csrf_exempt
@require_http_methods(["GET", "POST"])
def proxy_view(request, path):
    #extra_requests_args = {'params': request.GET.copy()}
    #extra_requests_args = {'data': request.POST.copy()}
    extra_requests_args = {'params': request.GET.copy(), 'data': request.POST.copy()}
    extra_requests_args['params']['token'] = _get_token()
    extra_requests_args['data']['token'] = _get_token()
    #request.method = 'POST'
    #extra_requests_args['params']['token'] = _get_token()
    print(extra_requests_args)
    print(path)
    #remoteurl = 'http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Network/USA/NAServer/Route/' + path
    #remoteurl = 'http://route.arcgis.com/arcgis/rest/services/World/ClosestFacility/NAServer/Route/' + path
    remoteurl = 'http://tasks.arcgisonline.com/ArcGIS/rest/services/NetworkAnalysis/ESRI_Route_NA/NAServer/Route/' + path
    #remoteurl = path
    # if remoteurl.find("?") > -1:
    #     remoteurl = remoteurl + "&token="+_get_token()
    # else:
    #     remoteurl = remoteurl + "?token="+_get_token()
    print(remoteurl)
    return proxy.views.proxy_view(request, remoteurl, extra_requests_args)

@csrf_exempt
@require_http_methods(["GET", "POST"])
def proxy_view_gp(request, path):
    extra_requests_args = {'params': request.GET.copy(), 'data': request.POST.copy()}
    #extra_requests_args = {'data': request.POST.copy()}
    #request.method = 'POST'
    #extra_requests_args['params']['token'] = _get_token()
    print(extra_requests_args)
    print(path)
    remoteurl = 'http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer/' + path
    #remoteurl = path
    # if remoteurl.find("?") > -1:
    #     remoteurl = remoteurl + "&token="+_get_token()
    # else:
    #     remoteurl = remoteurl + "?token="+_get_token()
    print(remoteurl)
    return proxy.views.proxy_view(request, remoteurl, extra_requests_args)


class MobileApp(TemplateView):
    template_name = 'mobile_app_demo.html'

    def get_context_data(self, **kwargs):
        context = super(MobileApp, self).get_context_data(**kwargs)
        return context

def reply_to_sms_messages(request):

    # u = User()
    # u.body = request.GET.get('Body')
    # u.number = request.GET.get('From')
    # u.time = datetime.datetime.now()
    # u.save()
    l = request.GET.get('From')
    r = HttpResponse('<Response><Sms>Thanks! Check out your nearest polling location! {}</Sms></Response>'.format('http://hackathon2014.azurewebsites.net/?p={}'.format(l)), 'text/XML')
    return r

def jamie_view(request):

    account_sid = settings.TWILIO_ACCOUNT_SID
    auth_token = settings.TWILIO_AUTH_TOKEN
    client = TwilioRestClient(account_sid, auth_token)

    smss = client.sms.messages.list()
    j_list = list()
    for x in smss:
        if x.to == u'51269':
            j_list.append({'body': x.body, 'number': x.from_})
    print j_list

    return HttpResponse(json.dumps(j_list), 'application/JSON')

    # j_list = list()
    #
    # for i in User.objects.order_by('-time').all():
    #     temp = dict()
    #     temp['body'] = i.body
    #     temp['number'] = i.number
    #     j_list.append(temp)
    # return HttpResponse(json.dumps(j_list), 'application/JSON')



def send_text(request):
    number = request.GET.get('n', None)
    body = request.GET.get('b', None)
    if number is None:
        return HttpResponse('')

    account_sid = settings.TWILIO_ACCOUNT_SID
    auth_token = settings.TWILIO_AUTH_TOKEN
    client = TwilioRestClient(account_sid, auth_token)

    message = client.messages.create(to=number,
                                     from_="51269",
                                     body=body,
                                     media_url="http://ec2-54-193-191-197.us-west-1.compute.amazonaws.com/static/images/Logo_Media.png")
    return HttpResponse('')