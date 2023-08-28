import { Link } from "react-router-dom";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

export default function NotFound(){
    return(
        <Segment>
            <Header icon>
                <Icon name='search'></Icon>
                Oops-we`ve looked everywhere but could not find what you are looking for!
            </Header>
            <Segment>
                <Segment.Inline>
                    <Button as={Link}>
                        Return to activties page
                    </Button>
                </Segment.Inline>
            </Segment>
        </Segment>
     
    )
}